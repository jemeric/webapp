const path = require("path");
const merge = require("webpack-merge");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

module.exports = (env) => {

    const isDevBuild = !(env && env.prod);
    
    const sharedConfig = () => ({
        mode: isDevBuild ? 'development' : 'production',
        resolve: {
            // Add '.ts' and '.tsx' as resolvable extensions.
            extensions: [".ts", ".tsx", ".js", ".json"]
        },
        output: {
            filename: "[name].js",
            publicPath: "dist/" // - Needed for server side and client-side path  Webpack dev middleware, if enabled, handles requests for this URL prefix
        },
        module: {
            rules: [
                // All files with a '.ts' or '.tsx' extension will be handled by 'awesome-typescript-loader'.
                { test: /\.tsx?$/, loader: "awesome-typescript-loader" },
                // All output '.js' files will have any sourcemaps re-processed by 'source-map-loader'.
                { enforce: "pre", test: /\.js$/, loader: "source-map-loader" }
            ]
        },
        // issue in SpaServices mean this is needed even if it is empty
        // if not there hot reload can't find updates
        plugins: []
    });

    const clientBundleConfig = merge(sharedConfig(), {
        entry: { "main-client": "./ClientApp/boot-client.tsx" },
        output: {
            path: path.join(__dirname, "./wwwroot/dist")
        },
        module: {
            rules: [
                {
                    test: /\.s?[ac]ss$/,
                    use: [
                        MiniCssExtractPlugin.loader,
                        // Interprets '@import' and '@url()' like 'import/required()' and will resolve them
                        { loader: 'css-loader', options: { url: false, sourceMap: true } },
                        // Loads a SASS/SCSS file and compiles it to CSS
                        { loader: 'sass-loader', options: { sourceMap: true } }
                    ]
                }
            ]
        },
        plugins: [
            new MiniCssExtractPlugin({
                filename: "style.css"
            })
        ],
        // When importing a module whose path matches one of the following, just
        // assume a corresponding global variable exists and use that instead.
        // This is important because it allows us to avoid bundling all of our
        // dependencies, which allows browsers to cache those libraries between builds.
        // TODO - only do this on client-side
        externals: {
            "react": "React",
            "react-dom": "ReactDOM"
        }
    });

    const serverBundleConfig = merge(sharedConfig(), {
        entry: { "main-server": "./ClientApp/boot-server.tsx" },
        output: {
            path: path.join(__dirname, "./ClientApp/dist"),
            libraryTarget: "commonjs" // get missing default error without this
        },

        // Enable sourcemaps for debugging webpack's output.
        devtool: "inline-source-map",
        target: "node"
    });

    return [clientBundleConfig, serverBundleConfig];
};