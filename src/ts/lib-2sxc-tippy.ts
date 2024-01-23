import tippy, { Placement } from 'tippy.js';
// import 'tippy.js/dist/tippy.css'; // optional for styling

interface TippyOptions {
    placement: Placement;
    interactive: Boolean;
    allowHTML: Boolean;

}

export function initTippy( { domClass, options } : { domClass: string, options: TippyOptions }) {
    document.querySelectorAll(`.${domClass}`).forEach((element) => {
        const fileId = element.id.split('-').pop();
        const contentElements = document.querySelectorAll(`.app-mobius-tooltip-content[data-file-id="${fileId}"]`);
      
        if (contentElements.length > 0) {
          const content = Array.from(contentElements).map((contentElement) => contentElement.innerHTML).join('');
          
          tippy(`#${element.id}`, {
            content: content, // From Div
            placement: options.placement,
            interactive: options.interactive as boolean,
            allowHTML: options.allowHTML as boolean,
          });
        }
      });
      
}

   