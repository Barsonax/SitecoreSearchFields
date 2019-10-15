# SitecoreSearchFields

Did you run into the limitations of the build in fields of sitecore when using buckets or a heavily search driven workflow for content editors? Is hard to find your items that are in buckets when using droplink or multilist fields? Would you wish you could have proper search enabled fields? 

Because I ran into these limitations I made these custom fields to deal with them. These custom fields provide the same search UI functionality that you normally get when using buckets. Everything works as you would expect such as:
- Basic search functionality
- Facets
- Bucket settings

Additionally you can set the persistent filter for a field by using the `pfilter` parameter in the source query. This will be added to the already existing persistent filter in the bucket settings.

# Setup
 
WIP

# Demo

SingleLinkField (similar to droplist but with search):
![singlelinkSample1](https://user-images.githubusercontent.com/19387223/66863388-30d4e380-ef93-11e9-9594-3fe0396e3017.gif)

MultiLinkField (yes I know we have a multilist with search but its not very usable)
![multilinkSample1](https://user-images.githubusercontent.com/19387223/66863390-329ea700-ef93-11e9-89ad-081105331e3b.gif)
