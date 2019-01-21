import * as React from "react";

interface IDrawToggleButtonProps {
  click: () => void;
}

const DrawerToggleButton = (props: IDrawToggleButtonProps) => (
  <button className="toggle-button" onClick={props.click}>
    <div className="toggle-button__line" />
    <div className="toggle-button__line" />
    <div className="toggle-button__line" />
    <div className="toggle-button__line" />
  </button>
);

export default DrawerToggleButton;
