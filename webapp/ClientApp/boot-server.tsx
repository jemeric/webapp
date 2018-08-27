import * as React from 'react';
import { createServerRenderer, RenderResult } from 'aspnet-prerendering';
import { renderToString } from 'react-dom/server';
import { Hello } from './components/Hello';

export default createServerRenderer(params => {
    return new Promise<RenderResult>((resolve, reject) => {
        const app = (<Hello compiler="Typescript" framework="React" />);

        resolve({
            html: renderToString(app)
        });
    });
});