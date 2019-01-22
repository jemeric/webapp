import './components/app.scss'; // needed for compilation to style.css
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { AppContainer } from 'react-hot-loader';
import { App } from './components/App';
import { BrowserRouter } from 'react-router-dom';

// entry point for react-scripts start (still need boot-client?)
function renderApp() {

    // This code starts up the React app when it runs in a browser.
    ReactDOM.hydrate(
        <AppContainer>
            <BrowserRouter>
                <App compiler="Typescript" framework="React" />
            </BrowserRouter>
        </AppContainer>,
        document.getElementById("example"));
}

renderApp();

// Allow Hot Module Replacement
if (module.hot) {
    module.hot.accept(() => {
        renderApp();
    });
}