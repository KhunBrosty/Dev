import 'dart:convert';
//import 'package:http/http.dart' as http;
import 'package:flutter_app/encryption/encrypt_request.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class Loginrepository {
  final String baseUrl = "https://localhost:7229";
  final _storage = FlutterSecureStorage();
  final _client = EncryptedHttpClient();

  Future<void> login(String username, String password) async {
    String url = '$baseUrl/login';
    final response = await _client.post(
      Uri.parse(url),
      headers: {'content-Type': 'application/json; charset=utf-8'},
      body: jsonEncode({'username': username, 'password': password}),
    );

    print(response.body);
    response.statusCode != 200 ? throw Exception('Failed to login') : null;

    final res = jsonDecode(response.body);
    final accessToken = res['accessToken'];
    final user = res['user'];

    accessToken == '' ? throw Exception('Failed to login') : null;

    await _storage.write(key: "access_token", value: accessToken);

    return user;
  }

  Future<void> logout() async {
    await _storage.delete(key: "access_token");
  }

  Future<void> getUser() async {
    String url = '$baseUrl/api/users';
    final response = await _client.get(
      Uri.parse(url),
      headers: {'content-Type': 'application/json; charset=utf-8'},
    );

    print(response.body);
    response.statusCode != 200 ? throw Exception('Failed to get users') : null;

    final res = jsonDecode(response.body);
    final accessToken = res['accessToken'];
    final user = res['user'];

    accessToken == '' ? throw Exception('Failed to login') : null;

    await _storage.write(key: "access_token", value: accessToken);

    return user;
  }
}
