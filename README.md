hbase-client
============

## .NET client libraries for HBase ##

- [Simple and easy to use](https://gist.github.com/TheTribe/7190398)
- [Lots of content](http://thetribe.github.io/hbase-client/) to get you started
- Available under the [FreeBSD](https://github.com/TheTribe/hbase-client/blob/master/LICENSE) license

## Status (MyGet builds) ##

- Master: [![hadoop-client/master MyGet Build Status](https://www.myget.org/BuildSource/Badge/hadoop-client?identifier=7b0811fb-6e6d-4ee3-9822-6bcb83c39a8d)](https://www.myget.org/gallery/hadoop-client)
- Next: [![hadoop-client/next MyGet Build Status](https://www.myget.org/BuildSource/Badge/hadoop-client?identifier=fa4240b4-8845-409d-8694-247f8872bc3b)](https://www.myget.org/gallery/hadoop-client)

## Development ##

Development is powered by [vagrant](http://www.vagrantup.com). To launch a virtual machine (Centos 6.4 x64) with HBase 0.98 REST server running,
run ``vagrant up`` from the root repository directory. Configuration for the instance is located in [the Hiera configuration YAML](.vagrant/hieradata/global.yaml).
Data is persistent until the virtual machine is destroyed.
