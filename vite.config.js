import { defineConfig } from 'vite';
import { resolve } from 'path';
import { fileURLToPath } from 'url';
import * as sass from 'sass';
import postcss from 'postcss';
import autoprefixer from 'autoprefixer';

const scssPath = resolve(process.cwd(), 'src/styles/index.scss');

// Cache SCSS dependencies so they stay watched even after compile errors
const scssFiles = new Set();

export default defineConfig({
  root: '.',
  cacheDir: '.vite_cache',

  build: {
    outDir: 'staging/dist',
    sourcemap: true,
    emptyOutDir: true,
    assetsDir: 'images',

    rollupOptions: {
      input: {
        scripts: resolve(process.cwd(), 'src/ts/index.ts'),
      },
      output: {
        entryFileNames: '[name].min.js',
        chunkFileNames: '[name].min.js',
        assetFileNames: (assetInfo) => {
          if (/\.(png|jpe?g|gif|svg|webp)$/i.test(assetInfo.name)) {
            return 'images/[name]-[hash][extname]';
          }
          return '[name]-[hash][extname]';
        },
      },
    },

    minify: 'esbuild',
    target: 'es2020',
    cssCodeSplit: false,

    // Polling required for watch mode on network drives (prevents UNKNOWN fs error)
    watch: {
      usePolling: true,
    },
  },

  resolve: {
    extensions: ['.ts', '.tsx', '.js', '.scss', '.css'],
  },

  plugins: [
    {
      name: 'compile-scss-to-css',

      buildStart() {
        // Register main SCSS file + all cached dependencies
        this.addWatchFile(scssPath);
        scssFiles.forEach(file => this.addWatchFile(file));
      },

      async generateBundle() {
        const result = sass.compile(scssPath, {
          sourceMap: true,
          style: 'compressed',
          silenceDeprecations: ['mixed-decls', 'color-functions', 'global-builtin', 'import'],
        });

        // Cache all imported SCSS files so they stay watched even after compile errors
        result.loadedUrls.forEach(url => {
          if (url.protocol === 'file:') {
            const filePath = fileURLToPath(url);
            scssFiles.add(filePath);
            this.addWatchFile(filePath);
          }
        });

        const postcssResult = await postcss([autoprefixer()]).process(result.css, {
          from: scssPath,
          to: 'styles.min.css',
          map: {
            inline: false,
            annotation: true,
            prev: result.sourceMap ? JSON.stringify(result.sourceMap) : false,
          },
        });

        this.emitFile({ type: 'asset', fileName: 'styles.min.css', source: postcssResult.css });
        if (postcssResult.map) {
          this.emitFile({ type: 'asset', fileName: 'styles.min.css.map', source: postcssResult.map.toString() });
        }
      },
    },
  ],

});