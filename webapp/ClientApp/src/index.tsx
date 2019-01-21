import * as React from "react";
import * as ReactDOM from "react-dom";

import { App } from "./components/App";

function renderApp() {
  ReactDOM.render(
    <App compiler="Typescript" framework="React" />,
    document.getElementById("example")
  );
}

renderApp();
