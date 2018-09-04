import * as React from 'react';
import { createServerRenderer, RenderResult } from 'aspnet-prerendering';
import { renderToString } from 'react-dom/server';
import { StaticRouter } from 'react-router-dom';
import { App } from './components/App';

export default createServerRenderer(params => {
    return new Promise<RenderResult>((resolve, reject) => {
        const app = (
            <StaticRouter>
                <App compiler="Typescript" framework="React" />
            </StaticRouter>
        );

        resolve({
            html: renderToString(app)
        });
    });
});