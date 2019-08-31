import * as React from "react";
import { Header } from "./Header";
import { useScrollYPosition } from "react-use-scroll-position";

const resetScroll = (isMobileNavOpen: boolean, contentScrollPosition: number) => {
  if(isMobileNavOpen) {
    window.scrollTo(0, 0);
  } else {
    window.scrollTo(0, contentScrollPosition);
  }
}

export function Layout(props: React.PropsWithChildren<{}>) {
  // const layoutRef = React.useRef<HTMLElement>(null);
  const [isMobileNavOpen, setIsMobileNavOpen] = React.useState(false);
  const [contentScrollPosition, setContentScrollPosition] = React.useState(0);
  const onMobileToggle = (e: boolean) => { 
    if(e !== isMobileNavOpen) setIsMobileNavOpen(e);
  };
  
  const scrollY = useScrollYPosition();
  const contentScrollOffset = isMobileNavOpen ? { top: 70 - contentScrollPosition } : {};
  const mobileNavContentClass = isMobileNavOpen ? "mobileNavOpened" : "";
  React.useEffect(() => {
    if(!isMobileNavOpen) {
      // store the most recent scroll state when the mobile nav was closed
      setContentScrollPosition(scrollY);
    }
  }, [scrollY]);

  React.useEffect(() => {
    resetScroll(isMobileNavOpen, contentScrollPosition);
    // Note** if useEffect returns a function React will run it when it's time to cleanup
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