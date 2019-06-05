const NPMExternals = require("../Assets/Json/NPMExternals.json");
const path = require("path");
const merge = require("webpack-merge");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CleanWebpackPlugin = require("clean-webpack-plugin");
const defaultOutput = "./dist";

module.exports = env => {
  const isDevBuild = !(env && env.prod);
  const isOutputOverride = env && env.outputPath;
  const outputPath = isOutputOverride ? env.outputPath : defaultOutput;
  const clientAssetPath = isOutputOverride ? "client" : "client/default";
  const serverAssetPath = isOutputOverride ? "server" : "server/default";
  // TODO - turns out this cleanup breaks the ASP.NET server-side rendering (not including it for default path for now)
  const doCleanBuild = isOutputOverride || !isDevBuild;
  const cssPlugin = new MiniCssExtractPlugin({
    filename: "style.css"
  });
  const clientPlugins = doCleanBuild
    ? [new CleanWebpackPlugin(), cssPlugin]
    : [cssPlugin]; // cleanup files in dist before doing build
  const serverPlugins = doCleanBuild ? [new CleanWebpackPlugin()] : [];

  const externals = NPMExternals.reduce(
    (obj, cur, i) => ((obj[cur.module] = cur.global), obj),
    {}
  );

  const sharedConfig = () => ({
    mode: isDevBuild ? "development" : "production",
    resolve: {
      // Add '.ts' and '.tsx' as resolvable extensions.
      extensions: [".ts", ".tsx", ".js", ".json"]
    },
    output: {
      filename: "[name].js",
      publicPath: "dist/" // - Needed for server side and client-side path  Webpack dev middleware, if enabled, handles requests for this URL prefix
    },
    module: {
      //rules: [
      //    // All files with a '.ts' or '.tsx' extension will be handled by 'awesome-typescript-loader'.
      //    { test: /\.tsx?$/, loader: "awesome-typescript-loader" },
      //    // All output '.js' files will have any sourcemaps re-processed by 'source-map-loader'.
      //    { enforce: "pre", test: /\.js$/, loader: "source-map-loader" }
      //]
      // UPDATED to use babel for typescript instead (see https://medium.com/@francesco.agnoletto/how-to-set-up-typescript-with-babel-and-webpack-6fba1b6e72d5)
      rules: [
        {
          test: /\.tsx?$/,
          loader: "babel-loader"
        },
        {
          test: /\.js$/,
          use: ["source-map-loader"],
          enforce: "pre"
        }
      ]
    },
    // issue in SpaServices mean this is needed even if it is empty
    // if not there hot reload can't find updates
    plugins: []
  });

  const clientBundleConfig = merge(sharedConfig(), {
    entry: { "main-client": "./src/boot-client.tsx" },
    output: {
      //path: path.join(__dirname, "../wwwroot/dist")
      path: path.join(__dirname, outputPath, clientAssetPath)
    },
    module: {
      rules: [
        {
          test: /\.s?[ac]ss$/,
          use: [
            MiniCssExtractPlugin.loader,
            // Interprets '@import' and '@url()' like 'import/required()' and will resolve them
            { loader: "css-loader", options: { url: false, sourceMap: true } },
            // Loads a SASS/SCSS file and compiles it to CSS
            { loader: "sass-loader", options: { sourceMap: true } }
          ]
        }
      ]
    },
    plugins: clientPlugins,
    // When importing a module whose path matches one of the following, just
    // assume a corresponding global variable exists and use that instead.
    // This is important because it allows us to avoid bundling all of our
    // dependencies, which allows browsers to cache those libraries between builds.
    // NOTE* only do this on client-side
    externals
  });

  const serverBundleConfig = merge(sharedConfig(), {
    entry: { "main-server": "./src/boot-server.tsx" },
    output: {
      path: path.join(__dirname, outputPath, serverAssetPath),
      libraryTarget: "commonjs" // get missing default error without this
    },
    plugins: serverPlugins,
    // Enable sourcemaps for debugging webpack's output.
    devtool: "inline-source-map",
    target: "node"
  });

  return [clientBundleConfig, serverBundleConfig];
};
