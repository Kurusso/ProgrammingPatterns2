
const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');

module.exports = {
    mode: "development",
    entry: './src/index.tsx',
    resolve: {
        extensions: ['.tsx', '.ts', '.js'],
    },
    output: {
        path: path.resolve(__dirname, 'build'),
        filename: 'bundle.js',
        publicPath: "/"
    },
    module: {
        rules: [
            {
                test: /\.(ts|tsx)$/,
                exclude: /node_modules/,
                use: ['ts-loader'],
            },
            {
                test: /\.css$/,
                use: ['style-loader', 'css-loader'],
            },
        ],
    },
    plugins: [
        new HtmlWebpackPlugin({
            template: './public/index.html',
        }),
         new CopyWebpackPlugin({
             patterns: [
                 { from: 'public', to: 'public' }
             ],
         })
    ],
    devServer: {
        static: {
            directory: path.join(__dirname, 'build'),
        },
        https: {
            key:'./cert/cert.key',
            cert:'./cert/cert.crt',
            ca: './cert/ca.crt',
        },
        compress: true,
        port: 3000,
        historyApiFallback: true,
    }
};
