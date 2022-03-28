RIOTLite is a free, open-source program that was written as a faster, better, and all-around more powerful version of the now-deprecated RIOT Scan software. The design of this program is single-purpose in nature: take a custom content coding dictionary and code texts with it.
Download

This software is completely free. If you really want to exchange money for this software, please consider donating what you can to a charity that you believe in.

__The latest build of RIOTLite can be downloaded from the "releases" link on this page.__

RIOTLite is currently only being developed for Windows. You will need to install Microsoft .NET 4.6.2 in order to run this program. You can download this version of .NET from the official Microsoft webpage, located here:

https://www.microsoft.com/en-us/download/details.aspx?id=53344

There is a good chance that this software could be compiled to run on non-Windows machines via Mono. However, I have not personally tried this yet.

## Dictionary File Formatting / Notes

RIOTLite is powered by your dictionaries, formatted in a LIWC-style fashion. Dictionary files are simply plain-text (.txt) files that have had their extensions changed to “.dic”. You can open and edit these files in any standard word processing software or text editor.

__Note__: You can find an example of Vasalou et al.’s (2011) Privacy Dictionary in this repository.
   
Some important notes about dictionary files:

Dictionary files can contain single-word or multi-word phrases

Dictionary files can use wildcards (\*) at any location within a word or phrase
For example, “g\*ness” will capture words like “goodness” and “greatness”

Dictionary files cannot have duplicated entries — each line should contain a single unique word or phrase

Precise matches are prioritized over wildcard matches. For example, if your dictionary contains the following entries:

```
    happy to be here    1    2
    \* to be here       3    4
```

…it will prioritize a match for “happy to be here” if an exact match is found.
