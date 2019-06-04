require("dotenv").config();
const gulp = require("gulp");
const spawn = require("child_process").spawn;
const AWS = require("aws-sdk");
const randomstring = require("randomstring");

// S3 uploads - https://derikwhittaker.blog/2018/02/10/package-and-publish-react-site-to-aws-s3-bucket-with-gulp/
// Conifugre AWS or DO credentials (https://docs.aws.amazon.com/cli/latest/userguide/cli-chap-configure.html)
const s3 = new AWS.S3({
  apiVersion: "2006-03-01",
  endpoint: process.env.S3_ENDPOINT
});

gulp.task("build-assets", function(done) {
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

function generateDeploymentVersion(done) {
  let version = randomstring.generate(7);
  var params = {
    Bucket: process.env.S3_BUCKET,
    Key: version
  };
  s3.headObject(params, function(err, metadata) {
    if (err && err.code === "NotFound") {
      // use this version
      done(version);
    } else {
      // try another version
      generateDeploymentVersion(done);
    }
  });
}

function uploadAssets(version, fromPath, toPath, permissions) {
  return spawn(
    "aws",
    [
      "s3",
      "cp",
      fromPath,
      `s3://${version}/${toPath}`,
      `--endpoint=https://${process.env.S3_BUCKET}.${process.env.S3_ENDPOINT}`,
      "--acl",
      permissions,
      "--recursive"
    ],
    { stdio: "inherit" }
  );
}

gulp.task("upload-assets", function(done) {
    generateDeploymentVersion(function(version) {
      uploadAssets(version, "./dist/client", "client", "public-read")
      .on("close", function() {
        uploadAssets(version, "./dist/server", "server", "private")
        .on("close", done);
      })
    });
});
