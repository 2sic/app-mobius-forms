export class App {
    constructor(moduleId: number) {
        // disable validate on the global asp.net form, to not interfere with the contact-form
        $("form").attr("novalidate", "");
    }

    public initialize() {
        console.log('Hallo Welt!');
    }
}
