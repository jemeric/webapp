import * as React from 'react';

interface BackdropProps {
    click: () => void;
}

const Backdrop: React.SFC<BackdropProps> = (props)  => (
    <div className="backdrop" onClick={props.click}></div>
);

export default Backdrop;