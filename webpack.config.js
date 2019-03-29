// Please enter your bootstrap version here. bs4 = bootstrap 4, bs3 = bootstrap 3.
const cssFramework = 'bs4';
// Enter the name of your app here. Use all lowercase lettering.
const appname = 'mobiusforms';

const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const path = require('path');

module.exports = env => {
  
  return {
    entry: ['./src/scss/' + cssFramework + '.scss', './src/ts/main.ts'],
    watch: true,
    mode: 'none',
    output: {
      path: path.resolve(__dirname, ((env && env.staging) ? 'staging/dist' : 'live/dist')),
      filename: 'app-bundle.min.js',
      library: appname,
    },
    resolve: {
      extensions: ['.ts', '.tsx', '.js', '.scss']
    },
    module: {
      rules: [{
          test: /\.scss$/i,
          use: [
            MiniCssExtractPlugin.loader,
            'css-loader',
            {
              loader: 'sass-loader',
            }
          ]
        },
        {
          test: /\.ts?$/,
          use: 'ts-loader',
        },
      ],
    },
    plugins: [
      new MiniCssExtractPlugin({
        filename: 'style.min.css'
      })
    ]
  };
};