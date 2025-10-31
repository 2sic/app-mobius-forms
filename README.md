<image src="app-icon.png" align="right" width="200px">

# Mobius Forms Builder 6 App for .net CMSs

> This is a [2sxc](https://2sxc.org) App for [DNN ☢️](https://www.dnnsoftware.com/) and [Oqtane 💧](https://www.oqtane.org/)

This **Mobius Forms** App is an add-on to DNN. It is _the most customizable Form extension_ in the DNN ecosystem.

| Aspect              | Status | Comments or Version
| ------------------- | :----: | -------------------
| 2sxc                | ✅    | requires 2sxc v19.00.00
| Dnn                 | ✅    | For v9.6.1
| Oqtane              | ✅    | Requires v5.00
| No jQuery           | ✅    |
| Live Demo           | ➖    |
| Install Checklist   | ✅    | See [Installation on azing.org](https://azing.org/2sxc)
| Source & License    | ✅    | included, ISC/MIT
| App Catalog         | ✅    | See [app catalog](https://2sxc.org/en/apps/app/mobius-forms-v5-with-mailchimp-recaptcha-polymorph-weback-and-more-hybrid-for-dnn-and-oqtane)
| Screenshots         | ✅    | See [app catalog](https://2sxc.org/en/apps/app/mobius-forms-v5-with-mailchimp-recaptcha-polymorph-weback-and-more-hybrid-for-dnn-and-oqtane)
| Best Practices      | ✅    | Uses v16.01 conventions
| Bootstrap 3         | ✅    | optimized
| Bootstrap 4         | ✅    | optimized
| Bootstrap 5         | ✅    | optimized

This means that it

1. can be used to create a simple contact form in one minute
2. can be modified to be any other form you need
3. can be used as a starting point for your own AJAX forms in DNN

The app is built with the [pattern **Don't be DAFT**][daft] (DAFT = Densely Abstract Features for Techies), aka the **Anti-Abstraction** pattern.
So customizing it is mostly done using common technologies like HTML, JS and some C#.

## Quick Intro To The Mobius Forms App for DNN

A [DNN App][2sxc] is like a DNN module, just way better 😉.
Since this is an open-code/open-source 2sxc-app, you can customize it to be anything you want!
This list just shows what it already does, so you know what you get out-of-the-first-box.

1. Pre-Built Forms for use or learning
    1. Basic contact form with _Subject, Message, Name, E-Mail_
    1. A support-request form with a dropdown-example
    1. An example with JS show/hide logic and saving raw JSON-data
1. Form builder to add/change fields as you wish without requiring development
1. Ability to completely re-program how fields are generated
1. **AJAX**, so no page reloads for validation, sending or messages
1. **Recaptcha** (optional) validation on client and server
1. data is saved, together with the _Timestamp_, _SenderIP_, optionaly generated _Title_ or even raw JSON-data
1. sends [various e-mails][cust-mail], which are **razor-templateable** and has **Reply-To** and **CC** options
1. **multi-language** labels and messages, already translated into English and German/Deutsch
1. field validation uses [html5 and pristinejs][pristine-js] and works with multiple forms on the same page
1. you can easily **review / manage / filter** the submitted items in a table-view
1. **export all submissions** into an Excel-compatible XML format
1. **open code C# WebApi** easy to customize if you ever need to

Because it's so simple and uses 2sxc, you can easily

* [translate it into any other language in minutes][translate]
* [add more fields, even with special validation in minutes][cust-field]
* [customize the e-mail templates][cust-mail]
* send more e-mails, trigger other [custom WebApi actions][cust-webapi]
* create [more custom forms][add-forms] which store into further content-types

## Get Started

This app is only useful if you use DNN or Oqtane. So assuming you have a DNN installation, all you need to do is install 2sxc and this app.

* Here's how to [install 2sxc and an App of your Choice](https://2sxc.org/en/apps/app/mobius-forms-v5-with-mailchimp-recaptcha-polymorph-weback-and-more-hybrid-for-dnn-and-oqtane)

* Now you can use this app as-is, or customize it to be whatever you need it to be.

* It probably helps to review the [Overview][overview] about how the parts play together by default, so you can then change as little as necessary to get it to do what you want

## Customize the App

The Source Code is all here - so you can easily customize to your hearts desire!

---

## History

* 2021-11
  * Updated to v12 best-practices
  * Removed all jQuery dependencies
  * Hybrid - now works in Dnn and Oqtane
* 2022-03
  * Added web.config with required assemblies
* v05.04.00 2022-04
  * Changed all access to services to ToSic.Sxc.Services
  * Changed instances of Edit.Enable to page.Activate()
  * Changed uses of the `Eav.Configuration.IFeaturesService` to `Sxc.Services.IFeaturesService`
  * Updated JS to use new webapi methods (fetch -> fetchRaw)
* v05.05.00 2022-06
  * Replaced all base classes with their 2sxc 14 equivalents
  * Replaced all GetService<> with the new ServiceKit14
  * Updated webpack
  * Updated all toolbar configurations to use the IToolbarService
* v05.05.01 2022-08
  * Fixed the warning messages for the forms
* v05.06.00 2023-05
  * Removed _ from Filenames
  * Replaced turnOn Tag with `Kit.Page.TurnOn`
  * Change Replace("p", ...) to Kit.Scrub()
  * New FieldBuilder for Checkboxes
* v05.07.00 2023-06
  * Updated to 16.02 conventions
  * All Razor code now fully typed
* v06.17.01
  * E-Mail Security improvement
* v06.17.02
  * Use new Builder.Kit.HtmlTags instance of Tag.Div
  * Refactor Code to functional
* v06.18.00
  * Finished 2sxc 17 Tags conventions (functional)
  * Added Tokens for default text to allow QueryString, User etc. properties
  * Made default-text field multiline (2 lines by default, stretchable)
* v06.19.00
  * Bugfixes
  * Replaced WebClient with HttpClient
  * SecureEndpoint for double-encryption
  * removed app web.config
* v06.20.00
  * Changed Recaptcha Namespace 
  * Added app web.config
* v06.21.00 
  * Refactored Mailchimp integration
  * Added Mailchimp Configuration for merge fields 
* v06.21.01
  * Bugfix category null value

[//]: # "Note: use full http-link, so we can copy/paste this from wiki to readme.md"
[2sxc]: https://2sxc.org/en/
[cust-field]: https://github.com/2sic/app-form-jquery-simple/wiki/Customize-Field
[cust-mail]: https://github.com/2sic/app-form-jquery-simple/wiki/Customize-Mails
[pristine-js]: https://github.com/sha256/Pristine
[add-forms]: https://github.com/2sic/app-form-jquery-simple/wiki/Add-Forms
[translate]: https://github.com/2sic/app-form-jquery-simple/wiki/Translate
[cust-webapi]: https://github.com/2sic/app-form-jquery-simple/wiki/Customize-WebApi
[daft]: http://2sxc.org/en/blog/post/the-dont-be-daft-pattern-densely-abstract-features-for-techies
[overview]: https://github.com/2sic/app-form-jquery-simple/wiki/Overview
