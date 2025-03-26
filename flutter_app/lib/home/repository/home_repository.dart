import 'dart:convert';

import 'package:flutter_app/encryption/encrypt_request.dart';
import 'package:flutter_app/home/model/home_model.dart';

class HomeRepository {
  final String baseUrl = "https://localhost:7229/home/todolist";
  final http = EncryptedHttpClient();

  Future<void> addDescription(String description) async {
    final response = await http.post(Uri.parse(baseUrl));

    response.statusCode != 200
        ? throw Exception('Failed to add description')
        : null;
    return;
  }

  Future<List<HomeModel>> getDescriptions() async {
    final response = await http.get(Uri.parse(baseUrl));

    response.statusCode != 200
        ? throw Exception('Failed to get descriptions')
        : null;

    final res = jsonDecode(response.body);
    return List<HomeModel>.from(res.map((x) => HomeModel.fromJson(x)));
  }
}
