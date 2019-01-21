import * as React from 'react';
import { NavLink } from "react-router-dom";

interface SideDrawerProps {
    show: boolean
}

const SideDrawer = (props: SideDrawerProps) => {
    let drawClasses = 'side-drawer';
    if(props.show) {
        drawClasses = 'side-drawer open';
    }
    return (
        <nav className={drawClasses}>
            <ul>
                <li>
                    <NavLink to="/" exact>Home</NavLink>
                </li>
                <li>
                    <NavLink to="/users" exact>Users</NavLink>
                </li>
            </ul>
        </nav>
    )
};

export default SideDrawer;