var gulp = require("gulp");
var spawn = require("child_process").spawn;

gulp.task("default", function(done) {
  // place code for default task here
  //console.log("NOTHING");
  // see https://stackoverflow.com/a/38552344
  // npm.cmd on windows? https://github.com/nodejs/node/issues/3675#issuecomment-154264390
  spawn(
    "npm.cmd",
    [
      "run",
      "build:prod",
      "-p",
      "--",
      "--env.outputPath=../../deploy-assets/dist"
    ],
    { cwd: "../webapp/ClientApp", stdio: "inherit" }
  ).on("close", done);
});
