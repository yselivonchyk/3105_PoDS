package com.main;

import java.net.InetAddress;
import java.net.UnknownHostException;
import java.util.Scanner;

public class Main {

	public static void main(String[] args) {
		Scanner input = new Scanner(System.in);
		System.out.println("Choose Version. 0-Client, 1-Server");
		int choice = input.nextInt();

		String memberIPandPort = "194.94.199.178:800";

		 if (choice!=0)
		new Server().launch();
		 else
		new Client().start(memberIPandPort);
		
		

		try {
			System.out.println("Your IP:" + InetAddress.getLocalHost());
		} catch (UnknownHostException e) {
			System.out.println("Error: " + e.getMessage());
		}

	}

}
