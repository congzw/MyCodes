echo off
cls

echo ����IP

netsh interface ip set address "��������" static 192.168.1.11 255.255.255.0 192.168.1.254 1

echo ������DNS

netsh interface ip set dns name="��������" static 192.168.16.253 primary

echo ******��ϲ�㣬�޸���ɣ�******

pause