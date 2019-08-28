import * as React from "react";
import { Header } from "./Header";
import { useScrollYPosition } from "react-use-scroll-position";

export interface ILayoutProps {
  drawerClickHandler: () => void;
}

const resetScroll = (isMobileNavOpen: boolean, contentScrollPosition: number) => {
  if(isMobileNavOpen) {
    window.scrollTo(0, 0);
  } else {
    // global.console.log("RESET SCROLL CORRECTLY: ", contentScrollPosition);
    window.scrollTo(0, contentScrollPosition);
  }
}

export function Layout(props: React.PropsWithChildren<{}>) {
  // const layoutRef = React.useRef<HTMLElement>(null);
  const [isMobileNavOpen, setIsMobileNavOpen] = React.useState(false);
  const [contentScrollPosition, setContentScrollPosition] = React.useState(0);
  const onMobileToggle = React.useCallback((e: boolean) => { 
    global.console.log("Mobile Nave Callback", e);
    setIsMobileNavOpen(e); 
  }, []);
  
  const scrollY = useScrollYPosition();
  const contentScrollOffset = isMobileNavOpen ? { top: 70 - contentScrollPosition } : {};
  const mobileNavContentClass = isMobileNavOpen ? "mobileNavOpened" : "";
  React.useEffect(() => {
    return () => {
      if(!isMobileNavOpen) {
        // store the most recent scroll state when the mobile nav was closed
        setContentScrollPosition(scrollY);
      }
    };
  }, [scrollY]);

  React.useEffect(() => {
    return () => {
      global.console.log("Is Mobile Nav Open: ", isMobileNavOpen);
      global.console.log("Content Scroll Position: ", contentScrollPosition);
      // both isMobileNavOpen and contentScrollPosition are getting the previously set values
      // seems like a bug? Hacky work-around reverse isMobilenavOpen since that works
      resetScroll(!isMobileNavOpen, contentScrollPosition);
    }
  }, [isMobileNavOpen]);

  return (
    <div>
      <Header mobileNavToggleHandler={onMobileToggle} />
      <main style={contentScrollOffset} className={mobileNavContentClass}>
        {props.children}
      </main>
    </div>
  );
}