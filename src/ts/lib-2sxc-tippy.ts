import tippy, { Placement } from "tippy.js";
// import 'tippy.js/dist/tippy.css'; // optional for styling

interface TippyOptions {
  placement: Placement;
  interactive: Boolean;
  allowHTML: Boolean;
}

export function initTippy({ domClass, options }: { domClass: string; options: TippyOptions }) {
  document.querySelectorAll(`.${domClass}`).forEach((element: Element) => {
    let content = "";
    const fileId = element.id.split("-").pop();
    let hasLength = false
    
    if (element.hasAttribute("data-content")) {
      content = (element as HTMLElement).dataset.content || "";
      const contentData = (element as HTMLElement).dataset.content;
      if (contentData && contentData.length > 10) hasLength = true;
    } else {
      const contentElements = document.querySelectorAll(`.app-mobius-tooltip-content[data-file-id="${fileId}"]`);
      if (contentElements.length > 0) {
        hasLength = true;
        content = Array.from(contentElements)
          .map((contentElement) => contentElement.innerHTML)
          .join("");
      }
    }
    if (hasLength) {
      tippy(`#${element.id}`, {
        content: content, // From Div
        placement: options.placement,
        interactive: options.interactive as boolean,
        allowHTML: options.allowHTML as boolean,
      });
    }
  });
}
