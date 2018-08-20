import * as React from "react";
import * as ReactDOM from "react-dom";

import { Hello } from "./components/Hello";

function renderApp() {
    ReactDOM.render(
        <Hello compiler="Typescript" framework="React" />,
        document.getElementById("example")
    );
}

renderApp();