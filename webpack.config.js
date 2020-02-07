// Please enter your bootstrap version here. bs4 = bootstrap 4, bs3 = bootstrap 3.
const cssFramework = 'bs4';
// Enter the name of your app here. Use all lowercase lettering.
const appname = 'mobiusforms';

const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const FriendlyErrorsWebpackPlugin = require('friendly-errors-webpack-plugin');
const WebpackBar = require('webpackbar');
const TerserPlugin = require('terser-webpack-plugin');
const OptimizeCSSAssetsPlugin = require('optimize-css-assets-webpack-plugin');

module.exports = env => {
  
  return {
    entry: ['./src/scss/' + cssFramework + '.scss', './src/ts/main.ts'],
    mode: 'production',
    devtool: 'source-map',
    watch: true,
    stats: {
      all: false,
      assets: true
    },
    output: {
      path: path.resolve(__dirname, 'staging/dist'),
      filename: 'app-bundle.min.js',
      library: appname,
    },
    resolve: {
      extensions: ['.ts', '.tsx', '.js', '.scss']
    },
    module: {
      rules: [{
          test: /\.scss$/i,
          exclude: /node_modules/,
          use: [
            MiniCssExtractPlugin.loader,
            {
              loader: 'css-loader',
              options: {
                  sourceMap: true
              }
            }, {
              loader: 'sass-loader',
              options: {
                  sourceMap: true
              }
            }
          ]
        },
        {
          test: /\.ts?$/,
          exclude: /node_modules/,
          use: 'ts-loader',
        },
      ],
    },
    optimization: {
      minimize: true,
      minimizer: [
        new TerserPlugin({
          terserOptions: {
            output: {
              comments: false,
            },
          },
          extractComments: false,
        }),
        new OptimizeCSSAssetsPlugin({ 
          cssProcessorOptions: { 
            map: { 
              inline: false, 
              annotation: true, 
            } 
          } 
        })
      ]
    },
    plugins: [
      new MiniCssExtractPlugin({
        filename: 'style.min.css'
      }),
      new WebpackBar(),
      new FriendlyErrorsWebpackPlugin(),
    ]
  };
};