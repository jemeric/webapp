const path = require("path");
const merge = require("webpack-merge");

module.exports = (env) => {

    const isDevBuild = !(env && env.prod);

    const sharedConfig = () => ({
        resolve: {
            // Add '.ts' and '.tsx' as resolvable extensions.
            extensions: [".ts", ".tsx", ".js", ".json"]
        },

        module: {
            rules: [
                // All files with a '.ts' or '.tsx' extension will be handled by 'awesome-typescript-loader'.
                { test: /\.tsx?$/, loader: "awesome-typescript-loader" },

                // All output '.js' files will have any sourcemaps re-processed by 'source-map-loader'.
                { enforce: "pre", test: /\.js$/, loader: "source-map-loader" }
            ]
        }
    });

    const clientBundleConfig = merge(sharedConfig(), {
        entry: { "main-client": "./ClientApp/boot-client.tsx" },
        output: {
            filename: "[name].js",
            path: path.join(__dirname, "./wwwroot/dist")
        },
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
            filename: "[name].js",
            path: path.join(__dirname, "./ClientApp/dist"),
            libraryTarget: "commonjs" // get missing default error without this
        },

        // Enable sourcemaps for debugging webpack's output.
        devtool: "inline-source-map",
        target: "node"
    });

    return [clientBundleConfig, serverBundleConfig];
};