# SitecoreSearchFields

Did you run into the limitations of the build-in fields in sitecore when using buckets or a heavily search driven workflow for content editors? Is it hard to find your items that are in buckets when using droplink or multilist fields? Would you wish you could have proper search enabled fields? 

Because I ran into these limitations I reverse engineered how sitecore uses the search UI and adapted it so I could made these custom search enabled fields. These custom fields provide the same search UI functionality that you normally get when using buckets so everything works as you would expect such as:
- Basic search functionality
- Facets
- Bucket settings

Additionally you can set some parameters in the source field of a field:
- id: the id of the item you want to search under
- pfilter: the persistent filter. This will be added to the already existing persistent filter in the bucket settings.
- more to be added...

Uses sitecore 9.2 but might work with other versions as well.

# Documentation
The documentation can be found in the [wiki](https://github.com/Barsonax/SitecoreSearchFields/wiki)
