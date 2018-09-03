import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { renderToString } from 'react-dom/server';
import { AppContainer } from 'react-hot-loader';
import { Hello } from './components/Hello';

function renderApp() {
    // This code starts up the React app when it runs in a browser.
    ReactDOM.hydrate(<AppContainer><Hello compiler="Typescript" framework="React" /></AppContainer>,
        document.getElementById("example"));
}

renderApp();

// Allow Hot Module Replacement
if (module.hot) {
    module.hot.accept(() => {
        renderApp();
    });
}