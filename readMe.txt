docker-compose

version: '3'

services:
  openldap:
    container_name: openldap
    image: osixia/openldap:latest
    ports:
      - "8389:389"
      - "8636:636"
    volumes:
      - ~/ldap/backup:/data/backup
      - ~/ldap/data:/var/lib/openldap
      - ~/ldap/config:/etc/openldap/slapd.d
      - ~/ldap/certs:/assets/slapd/certs
    command: [--copy-service,  --loglevel, debug]
  phpldapadmin:
    container_name: phpldapadmin
    image: osixia/phpldapadmin:latest
    ports:
      - "8080:80"
    environment:
      - PHPLDAPADMIN_HTTPS="false"
      - PHPLDAPADMIN_LDAP_HOSTS=openldap
    links:
      - openldap
    depends_on:
      - openldap



cmd : 
	docker-compose up
	多個 container run 起來

ldap server : 
	localhost:8389
phpLdapAdmin :
	localhost:8080
