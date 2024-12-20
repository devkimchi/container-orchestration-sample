package com.devkimchi.eshop.lite.weatherapi.controllers;

import java.time.LocalDate;
import java.util.Random;
import java.util.stream.IntStream;

import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.devkimchi.eshop.lite.weatherapi.models.WeatherForecast;

@RequestMapping(value = "/api")
@RestController
public class WeatherController {

    private final String[] summaries = { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

    @GetMapping(value = "/weatherforecast", produces = MediaType.APPLICATION_JSON_VALUE)
    public WeatherForecast[] getWeatherForecast() {
        Random random = new Random();
        return IntStream.range(1, 6)
                .mapToObj(i -> new WeatherForecast(
                        LocalDate.now().plusDays(i),
                        random.nextInt(75) - 20,
                        summaries[random.nextInt(summaries.length)]))
                .toArray(WeatherForecast[]::new);
    }

}
