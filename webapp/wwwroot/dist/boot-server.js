"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var React = require("react");
var aspnet_prerendering_1 = require("aspnet-prerendering");
var server_1 = require("react-dom/server");
var Hello_1 = require("./components/Hello");
exports.default = aspnet_prerendering_1.createServerRenderer(function (params) {
    return new Promise(function (resolve, reject) {
        var app = (React.createElement(Hello_1.Hello, { compiler: "Typescript", framework: "React" }));
        resolve({
            html: server_1.renderToString(app)
        });
    });
});
//# sourceMappingURL=boot-server.js.map