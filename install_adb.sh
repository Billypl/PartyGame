#!/bin/bash


sudo apt update && \
	sudo apt install -y wget unzip openjdk-21-jdk && \ 
	wget https://dl.google.com/android/repository/commandlinetools-linux-11076708_latest.zip && \
	mkdir -p ~/Android/Sdk/ && \
	unzip commandlinetools-linux-11076708_latest.zip -d ~/Android/Sdk && \
	rm commandlinetools-linux-11076708_latest.zip && \
	mkdir -p ~/Android/Sdk/cmdline-tools/latest
mv ~/Android/Sdk/cmdline-tools/* ~/Android/Sdk/cmdline-tools/latest && \
	echo -e "export ANDROID_HOME=$HOME/Android/Sdk\nexport PATH=\$PATH:\$ANDROID_HOME/platform-tools\nexport PATH=\$PATH:\$ANDROID_HOME/cmdline-tools/latest/bin" >> ~/.bashrc && \
	source ~/.bashrc && \
	if [ -f ~/.zshrc ]; then echo -e "export ANDROID_HOME=$HOME/Android/Sdk\n\nexport PATH=\$PATH:\$ANDROID_HOME/platform-tools\nexport PATH=\$PATH:\$ANDROID_HOME/tools/bin" >> ~/.zshrc;
		omz reload; fi;
		sdkmanager "platform-tools"
		adb version
		sdkmanager --list
