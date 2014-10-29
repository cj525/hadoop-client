# -*- mode: ruby -*-
# vi: set ft=ruby :
require 'yaml'

hbase_port = YAML.load_file('.vagrant/hieradata/global.yaml')['hbase::port']

Vagrant.configure('2') do |config|
  config.vm.box = 'centos-64-x64-vbox4210'
  config.vm.box_url = 'http://puppet-vagrant-boxes.puppetlabs.com/centos-64-x64-vbox4210.box'
  config.vm.hostname = 'vagrant-hbase.mydomain.local'

  config.vm.provider :virtualbox do |vb|
    vb.customize [
      # Key                Value
      'modifyvm',          :id, 
      '--cpuexecutioncap', '90',
      '--memory',          '1376',
      '--nictype2',        'virtio',
    ]
    config.vm.network :forwarded_port, guest: hbase_port, host: hbase_port

  end

  config.vm.network :private_network, 
    :ip => '192.168.55.100',
    :netmask => '255.255.255.0',
    :nictype => 'virtio'

  config.vm.provision 'shell' do |shell|
    shell.inline = 
      "echo $'---\n:backends:\n  - yaml\n:yaml:\n  :datadir: /etc/puppet/hieradata\n:hierarchy:\n  - global' > /etc/puppet/hiera.yaml;" +
      '[[ -d /etc/puppet/modules/hbase ]] || sudo puppet module install myoung34-hbase;' +
      'rm -rf /etc/puppet/hieradata;' +
      'cp -r /vagrant/.vagrant/hieradata /etc/puppet;'
  end

  config.vm.provision :puppet do |puppet|
    puppet.options = [
      '--hiera_config=/etc/puppet/hiera.yaml', 
    ]
    puppet.manifests_path = '.vagrant/manifests'
    puppet.manifest_file = 'default.pp'
  end

end
