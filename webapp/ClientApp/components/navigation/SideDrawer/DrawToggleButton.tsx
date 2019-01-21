import * as React from 'react';

interface DrawToggleButtonProps {
    click: () => void;
}

const DrawerToggleButton = (props: DrawToggleButtonProps) => (
    <button className="toggle-button" onClick={props.click}>
        <div className="toggle-button__line" />
        <div className="toggle-button__line" />
        <div className="toggle-button__line" />
        <div className="toggle-button__line" />
    </button>
);

export default DrawerToggleButton;