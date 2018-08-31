import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { renderToString } from 'react-dom/server';
import { Hello } from './components/Hello';

function renderApp() {
    // This code starts up the React app when it runs in a browser.
    ReactDOM.hydrate(<Hello compiler="Typescript2" framework="React" />,
        document.getElementById("example"));
}

renderApp();