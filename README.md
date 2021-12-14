<image src="app-icon.png" align="right" width="200px">

# Mobius Forms 5 App for .net CMSs

> This is a [2sxc](https://2sxc.org) App for [DNN ☢️](https://www.dnnsoftware.com/) and [Oqtane 💧](https://www.oqtane.org/)

This **Mobius Forms** App is an add-on to DNN. It is _the most customizable Form extension_ in the DNN ecosystem.

| Aspect              | Status | Comments or Version |
| ------------------- | :----: | ------------------- |
| 2sxc                | ✅    | requires 2sxc v12.05
| Dnn                 | ✅    | For v7, v8 and v9
| Oqtane 2            | ✅    | Requires v2.02
| No jQuery           | ?    | 
| Live Demo           | ➖    |
| Install Checklist   | ✅    | See [Installation] on [azing.org](https://azing.org/2sxc)
| Source & License    | ✅    | included, ISC/MIT
| App Catalog         | ✅    | See [app catalog](https://2sxc.org/en/apps/app/mobius-forms-v5-with-mailchimp-recaptcha-polymorph-weback-and-more-hybrid-for-dnn-and-oqtane)
| Screenshots         | ✅    | See [app catalog](https://2sxc.org/en/apps/app/mobius-forms-v5-with-mailchimp-recaptcha-polymorph-weback-and-more-hybrid-for-dnn-and-oqtane)
| Best Practices      | ✅    | Uses v12.05 conventions
| Bootstrap 3         | ✅    | optimized
| Bootstrap 4         | ✅    | optimized
| Bootstrap 5         | ✅    | optimized

This means that it

1. can be used to create a simple contact form in one minute
2. can be modified to be any other form you need
4. can be used as a starting point for your own AJAX forms in DNN

The app is built with the [pattern **Don't be DAFT**][daft] (DAFT = Densely Abstract Features for Techies), aka the **Anti-Abstraction** pattern. 
So customizing it is mostly done using common technologies like HTML, JS and some C#. 

## Quick Intro To The Mobius Forms App for DNN
A [DNN App][2sxc] is like a DNN module, just way better :). Since this is an open-code/open-source 2sxc-app, you can customize it to be anything you want! This list just shows what it already does, so you know what you get out-of-the-first-box.

1. Pre-Built Forms for use or learning
    1. Basic contact form with _Subject, Message, Name, E-Mail_
    1. A support-request form with a dropdown-example
    1. An example with JS show/hide logic and saving raw JSON-data
1. **AJAX**, so no page reloads for validation, sending or messages
1. **Recaptcha** (optional) validation on client and server
2. data is saved, together with the _Timestamp_, _SenderIP_, optionaly generated _Title_ or even raw JSON-data
3. sends [various e-mails][cust-mail], which are **razor-templateable** and has **Reply-To** and **CC** options
4. **multi-language** labels and messages, already translated into English and German/Deutsch
6. field validation uses [html5 and jQueryValidation][jqval] and works with multiple forms on the same page
7. you can easily **review / manage / filter** the submitted items in a table-view
8. **export all submissions** into an Excel-compatible XML format
1. **open code C# WebApi** easy to customize if you ever need to

Because it's so simple and uses 2sxc, you can easily
* [translate it into any other language in minutes][translate]
* [add more fields, even with special validation in minutes][cust-field]
* [customize the e-mail templates][cust-mail]
* send more e-mails, trigger other [custom WebApi actions][cust-webapi]
* create [more custom forms][add-forms] which store into further content-types

## Getting Started

This app is only useful is you use DNN. So assuming you have a DNN installation, all you need to do is install 2sxc and this app. 

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


[//]: # "Note: use full http-link, so we can copy/paste this from wiki to readme.md"
[2sxc]: https://2sxc.org/en/
[cust-field]: https://github.com/2sic/app-form-jquery-simple/wiki/Customize-Field
[cust-mail]: https://github.com/2sic/app-form-jquery-simple/wiki/Customize-Mails
[jqval]: https://jqueryvalidation.org/
[add-forms]: https://github.com/2sic/app-form-jquery-simple/wiki/Add-Forms
[translate]: https://github.com/2sic/app-form-jquery-simple/wiki/Translate
[cust-webapi]: https://github.com/2sic/app-form-jquery-simple/wiki/Customize-WebApi
[daft]: http://2sxc.org/en/blog/post/the-dont-be-daft-pattern-densely-abstract-features-for-techies
[overview]: https://github.com/2sic/app-form-jquery-simple/wiki/Overview