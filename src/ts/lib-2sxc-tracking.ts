/*
  This is a shared code used in various 2sxc apps. Make sure that they are in sync, so if you improve it, improve all 2sxc apps which use this. 
  ATM they are:
  - EventsAndCourses6
  - MobiusForms5
  The master with the newest / best version must always be the Gallery7, the others should then get a fresh copy.
  Because this is shared, all parameters like DOM-IDs etc. must be provided in the Init-call that it can work across apps
*/ 

export function addTrackingEvent(trackingEventName: string, trackingCategory: string, trackingLabel: string) {
  const trackingEvent = new CustomEvent(trackingEventName, { detail: { category: trackingCategory, action: 'submit', label: trackingLabel } });
  document.dispatchEvent(trackingEvent);
}