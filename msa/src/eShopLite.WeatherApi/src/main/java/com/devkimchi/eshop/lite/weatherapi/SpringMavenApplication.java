package com.devkimchi.eshop.lite.weatherapi;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.context.annotation.ComponentScan;

@SpringBootApplication
@ComponentScan({"com.devkimchi.eshop.lite.weatherapi.controllers"})
public class SpringMavenApplication {
    public static void main(String[] args) {
		SpringApplication.run(SpringMavenApplication.class, args);
	}

}
