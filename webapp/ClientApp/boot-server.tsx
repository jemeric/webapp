import * as React from 'react';
import { createServerRenderer, RenderResult } from 'aspnet-prerendering';
import { renderToString } from 'react-dom/server';
import { StaticRouter } from 'react-router-dom';
import { App } from './components/App';

export default createServerRenderer(params => {
    return new Promise<RenderResult>((resolve, reject) => {
        // this context object contains the results of the render (i.e. context.url will contain URL to redirect to if <Redirect> was used)
        const routerContext: any = {};

        const app = (
            <StaticRouter context={routerContext} location={params.location.path}>
                <App compiler="Typescript" framework="React" />
            </StaticRouter>
        );

        // If there's a redirection, just send this information back to the host application
        if (routerContext.url) {
            resolve({ redirectUrl: routerContext.url });
            return;
        }

        resolve({
            html: renderToString(app)
        });
    });
});