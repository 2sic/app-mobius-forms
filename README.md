[//]: # "*******************************************************"
[//]: # "Read this first!!! "
[//]: # "This text should always be identical in the Wiki/Home and Readme.md"
[//]: # "So always update it first in the wiki, then copy-paste everything into readme.md"
[//]: # "*******************************************************"

## Quick Intro To The jQuery Simple Form app
This [2sxc][2sxc] app serves 4 core purposes

1. create a contact form in any DNN web site in 1 minute
2. easy to customize with more/custom input fields as you need
3. create many same or different forms in your site
4. use it as a starting point to create your own online AJAX forms in DNN

## Already Implemented Features
Since this is a 2sxc-app, you can customize it to be anything you want! This list just shows what it already does, so you know what you get out-of-the-first-box.

1. Basic contact form with Subject, Message, Name, E-Mail
2. This is saved, together with the timestamp and SenderIP
3. It sends [various e-mails][cust-mail], which are razor-templateable and has Reply-To and CC options
4. All labels and messages are multi-language, and translated into English and German/Deutsch
5. All interactions are AJAX / WebAPI, so no postbacks etc.
6. Form validation uses [html5 and jQueryValidation][jqval]
7. You can easily review / manage / filter the submitted items in a table-view
8. export all submissions into an Excel-compatible XML format

Because it's so simple and uses 2sxc, you can easily
* [translate it into any other language in minutes][translate]
* [add more fields, even with special validation in minutes][cust-field]
* [customize the e-mail templates][cust-mail]
* send more e-mails, trigger other [custom WebApi actions][cust-webapi]
* create [more custom forms][add-forms] which store into further content-types

## Getting Started
This app is only useful is you use DNN. So assuming you have a DNN installation, all you need to do is install 2sxc and this app. 

* Here's how to [install 2sxc and an App of your Choice](http://2sxc.org/en/blog/post/install-2sxc-and-an-app-of-your-choice)

Now you can use this app as-is, or customize it to be whatever you need it to be. 


## Credits
The icon was built using the CC icon [_optical illusion by pedro baños cancer_ from the Noun Project](https://thenounproject.com/term/mobius-strip/182380/)


[//]: # "Note: use full http-link, so we can copy/paste this from wiki to readme.md"
[2sxc]: https://2sxc.org/en/
[cust-field]: https://github.com/2sic/app-form-jquery-simple/wiki/Customize-Field
[cust-mail]: https://github.com/2sic/app-form-jquery-simple/wiki/Customize-Mails
[jqval]: https://jqueryvalidation.org/
[add-forms]: https://github.com/2sic/app-form-jquery-simple/wiki/Add-Forms
[translate]: https://github.com/2sic/app-form-jquery-simple/wiki/Translate
[cust-webapi]: https://github.com/2sic/app-form-jquery-simple/wiki/Customize-WebApi