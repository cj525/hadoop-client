# hbase-client Changelog #

The following changelog details the changes made with each release of hbase-client.

## 1.0.0 ##
Authors of this release: [John Batte](https://github.com/jbatte47), [Christoper Wade](https://github.com/chrislwade)

- Update references to RestSharp.Injection ([Issue 68](https://github.com/TheTribe/hbase-client/issues/68))
- Switch to pure container references ([Issue 52](https://github.com/TheTribe/hbase-client/issues/52))
- Change MaxResults to the correct name: MaxVersions ([Issue 63](https://github.com/TheTribe/hbase-client/issues/63))
- Update references to Autofac ([Issue 61](https://github.com/TheTribe/hbase-client/issues/61))


## 1.0-beta.5 ##
Authors of this release: [John Batte](https://github.com/jbatte47), [Christoper Wade](https://github.com/chrislwade)

- Fixed exception when `FindCells` is called on an empty table ([Issue 47](https://github.com/TheTribe/hbase-client/issues/47))
- Fixed hidden connection errors ([Issue 41](https://github.com/TheTribe/hbase-client/issues/41))
- Fixed failure with `CreateTable` + empty `ColumnSchema.Name` ([Issue 48](https://github.com/TheTribe/hbase-client/issues/48))
- Updated 3rd party dependencies ([Issue 49](https://github.com/TheTribe/hbase-client/issues/49))
- Added table name to cells in results ([Issue 46](https://github.com/TheTribe/hbase-client/issues/46))

## 1.0-beta.4 ##
Author of this release: [John Batte](https://github.com/jbatte47)

- Updated configuration types to return `false` from `IsReadOnly` ([Issue 42](https://github.com/TheTribe/hbase-client/issues/42))
- Updated `Stargate` to use all-synchronous calls for its synchronous methods

## 1.0-beta.3 ##
Author of this release: [John Batte](https://github.com/jbatte47)

- Added ability to perform a "fuzzy match" between `Identifier` instances ([Issue 34](https://github.com/TheTribe/hbase-client/issues/34))
- Added ability to extract single values from `CellSet` instances ([Issue 35](https://github.com/TheTribe/hbase-client/issues/35))
- Refactored `Identifier` and model extensions ([Issue 33](https://github.com/TheTribe/hbase-client/issues/33))

## 1.0-beta.2 ##
Author of this release: [John Batte](https://github.com/jbatte47)

- Table administration support ([Issue 11](https://github.com/TheTribe/hbase-client/issues/11))
 - Create a table
 - Enumerate all tables
 - Delete a table
- Added scanner support ([Issue 12](https://github.com/TheTribe/hbase-client/issues/12))
 - Create a scanner
 - Filter support: `ColumnPrefixFilter`, `ColumnRangeFilter`, `FamilyFilter`,  
`QualifierFilter`, `RowFilter`, `FirstKeyOnlyFilter`, `InclusiveStopFilter`,  
`MultipleColumnPrefixFilter`, `PageFilter`, `PrefixFilter`, `SingleColumnValueFilter`,  
`TimestampsFilter`, `FilterList`
 - Read a result from a scanner (Manual or via disposable/enumerable object)
 - Delete a scanner (Manual or via disposable/enumerable object)
- Fixed NuGet package generation issues ([Issue 22](https://github.com/TheTribe/hbase-client/issues/22))

## 1.0-beta.1 ##
Authors of this release: [John Batte](https://github.com/jbatte47), [David Brandon](https://github.com/binaryberserker)

- Basic API model
- XML payload support
- Domain-aware resource URI builder
- Extensible REST response error provider
- Programmatic and configuration-based setup
- Autofac integration
