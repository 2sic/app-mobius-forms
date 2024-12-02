const path = require("path");
const webpack = require("webpack");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const ForkTsCheckerWebpackPlugin = require('fork-ts-checker-webpack-plugin');
const TerserPlugin = require('terser-webpack-plugin');

module.exports = (env) => {
  let lastMessage = '';
  
  return {
    mode: "production",
    entry: {
      styles: `./src/styles/bs5.scss`,
      scripts: "./src/ts/index.ts",
    },
    output: {
      path: path.resolve(__dirname, `staging/dist`),
      filename: "[name].min.js",
    },
    devtool: "source-map",
    watch: true,
    cache: {
      type: "filesystem",
      cacheDirectory: path.resolve(__dirname, ".temp_cache"),
      compression: "gzip",
    },
    resolve: {
      extensions: ['.ts', '.js', '.scss']
    },
    plugins: [
      new MiniCssExtractPlugin({
        filename: '[name].min.css',
      }),
      new webpack.ProgressPlugin((percentage, message) => {
        const progress = Math.round(percentage * 100);
        const progressBar = `[${'='.repeat(progress / 2)}${' '.repeat(50 - progress / 2)}]`;
        if (message !== lastMessage) {
          console.log(`${progress}% ${progressBar} ${message}`);
          lastMessage = message;
        }
      }),
      new ForkTsCheckerWebpackPlugin({
        async: false, // Blocks the build on type errors
        typescript: {
          configFile: path.resolve(__dirname, 'tsconfig.json'),
        },
      }),
    ],
    optimization: {
      splitChunks: {
        chunks: "all",
      },
      minimize: true, // Ensure minification is enabled
      minimizer: [
        new TerserPlugin({
          terserOptions: {
            format: {
              comments: false, // Removes all comments
            },
          },
          extractComments: false, // Disables .LICENSE.txt file generation
        }),
      ],
    },
    module: {
      rules: [
        {
          test: /\.(s[ac]|c)ss$/,
          exclude: /node_modules/,
          use: [
            MiniCssExtractPlugin.loader,
            {
              loader: "css-loader",
              options: {
                sourceMap: true,
              },
            },
            {
              loader: "postcss-loader",
              options: {
                sourceMap: true,
                postcssOptions: {
                  plugins: function () {
                    return [
                      require('autoprefixer'),
                      require('cssnano')({
                        preset: 'default',
                      }),
                    ];
                  }
                }
              },
            },
            {
              loader: "sass-loader",
              options: {
                sourceMap: true,
                sassOptions: {
                  silenceDeprecations: ['mixed-decls', 'color-functions', 'global-builtin', 'import'],
                }
              },
            },
          ],
        },
        {
          test: /\.tsx?$/,
          use: [
            {
              loader: 'babel-loader', // Babel for transformations
              options: {
                presets: [
                  '@babel/preset-env',
                  '@babel/preset-typescript',
                ],
                plugins: [
                  '@babel/plugin-transform-class-properties',
                  '@babel/plugin-transform-object-rest-spread',
                ],
              }
            },
            {
              loader: 'ts-loader', // Type-checking
              options: {
                transpileOnly: true, // Skip type-checking; separate type-checker recommended
              },
            },
          ],
          exclude: /node_modules/,
        }
      ],
    },
  };
};
