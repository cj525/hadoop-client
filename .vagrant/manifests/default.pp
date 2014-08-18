$hbase_port = hiera('hbase_port')

yumrepo { 'HDP-2.1.4.0':
  baseurl         => 'http://public-repo-1.hortonworks.com/HDP/centos6/2.x/updates/2.1.4.0',
  descr           => 'Hortonworks Data Platform Version - HDP-2.1.4.0',
  enabled         => 1,
  gpgcheck        => 0,
  gpgkey          => 'http://public-repo-1.hortonworks.com/HDP/centos6/2.x/updates/2.1.4.0/RPM-GPG-KEY/RPM-GPG-KEY-Jenkins',
  metadata_expire => 10,
  priority        => 1,
}->

yumrepo { 'HDP-UTILS-1.1.0.17':
  baseurl         => 'http://public-repo-1.hortonworks.com/HDP-UTILS-1.1.0.17/repos/centos6',
  descr           => 'Hortonworks Data Platform Utils Version - HDP-UTILS-1.1.0.17',
  enabled         => 1,
  gpgcheck        => 0,
  gpgkey          => 'http://public-repo-1.hortonworks.com/HDP/centos6/2.x/updates/2.1.4.0/RPM-GPG-KEY/RPM-GPG-KEY-Jenkins',
  metadata_expire => 10,
  priority        => 1,
}->

service { 'iptables':
  ensure => 'stopped',
}->

package { 'dos2unix':
  ensure => installed,
}->

package { 'java-1.7.0-openjdk.x86_64':
  ensure   => present,
  provider => yum,
}->

package { 'hbase':
  ensure   => latest,
  provider => yum,
}->

file { '/etc/init.d/hbase':
  content => template('/vagrant/.vagrant/templates/init_d_hbase.erb'),
  replace => true,
}->

# Git can mess up line endings.
exec { 'fix initd':
  command   => 'sudo chmod +x /etc/init.d/hbase; sudo dos2unix /etc/init.d/hbase',
  notify    => Service['hbase'],
  path      => $::path,
  require   => Package['dos2unix'],
  subscribe => File['/etc/init.d/hbase'],
}

file { '/etc/profile.d/hbase.sh':
  content => template('/vagrant/.vagrant/templates/profile_d_hbase.sh.erb'),
  replace => true,
}->

# Git can mess up line endings.
exec { 'fix profiled':
  command   => 'sudo chmod +x /etc/profile.d/hbase.sh; sudo dos2unix /etc/profile.d/hbase.sh',
  path      => $::path,
  require   => Package['dos2unix'],
  subscribe => File['/etc/profile.d/hbase.sh'],
}

service { 'hbase':
  ensure => running,
}
