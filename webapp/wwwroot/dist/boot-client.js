"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var React = require("react");
var ReactDOM = require("react-dom");
var Hello_1 = require("./components/Hello");
function renderApp() {
    // This code starts up the React app when it runs in a browser.
    ReactDOM.hydrate(React.createElement(Hello_1.Hello, { compiler: "Typescript2", framework: "React" }), document.getElementById("example"));
}
renderApp();
//# sourceMappingURL=boot-client.js.map