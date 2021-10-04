// so it can be called from the HTML when content re-initializes dynamically
const winAny = (window as any);
winAny.appMobius5 ??= {};
winAny.appMobius5.init ??= initAppMobius5;

function initAppMobius5({ domId } : { domId: string }) {

}