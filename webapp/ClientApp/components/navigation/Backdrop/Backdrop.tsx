import * as React from "react";

interface IBackdropProps {
  click: () => void;
}

const Backdrop: React.SFC<IBackdropProps> = props => (
  <div className="backdrop" onClick={props.click} />
);

export default Backdrop;
