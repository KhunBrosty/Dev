import 'package:http/http.dart' as http;
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:encrypt/encrypt.dart';
import 'package:jwt_decoder/jwt_decoder.dart';
import 'dart:convert';

class EncryptedHttpClient extends http.BaseClient {
  final http.Client _inner = http.Client();
  final _storage = FlutterSecureStorage();
  final String keyString = 'AyoZNg948mTYJ6uJSlzxcbonCboWYhKL';
  final String ivString = 'Z163xZu5gheONuff';

  String encryptData(String plainText) {
    final key = Key.fromUtf8(keyString);
    final iv = IV.fromUtf8(ivString);
    final encrypter = Encrypter(AES(key, mode: AESMode.cbc, padding: 'PKCS7'));

    final encrypted = encrypter.encrypt(plainText, iv: iv);
    return encrypted.base64;
  }

  dynamic decryptData(String encryptedText) {
    final key = Key.fromUtf8(keyString);
    final iv = IV.fromUtf8(ivString);
    final encrypter = Encrypter(AES(key, mode: AESMode.cbc, padding: 'PKCS7'));
    final i = encrypter.decrypt64(encryptedText, iv: iv);

    return jsonDecode(i);
  }

  dynamic getToken() async {
    final accessToken = await _storage.read(key: 'access_token');
    final refreshToken = await _storage.read(key: 'refresh_token');

    if (refreshToken != null && JwtDecoder.isExpired(refreshToken)) {
      await _storage.delete(key: 'access_token');
      await _storage.delete(key: 'refresh_token');
    }

    if (accessToken != null && JwtDecoder.isExpired(accessToken)) {
      final refreshToken = await _storage.read(key: 'refresh_token');
      if (refreshToken != null) {
        final response = await _inner.post(
          Uri.parse('https://localhost:7229/api/refresh-token'),
          headers: {'Content-Type': 'application/json'},
          body: jsonEncode({'refreshToken': refreshToken}),
        );

        if (response.statusCode == 200) {
          final res = jsonDecode(response.body);
          await _storage.write(key: 'access_token', value: res['accessToken']);
          return res['accessToken'];
        } else {
          throw Exception('Failed to refresh token');
        }
      } else {
        throw Exception('Refresh token not found');
      }
    } else {
      return accessToken;
    }
  }

  @override
  Future<http.StreamedResponse> send(http.BaseRequest request) async {
    if (request is http.Request && request.body.isNotEmpty) {
      final encryptedBody = encryptData(request.body);
      request.body = jsonEncode({'data': encryptedBody});
    }

    final response = await _inner.send(request);

    final responseBody = await http.Response.fromStream(response);
    final decryptedBody = decryptData(jsonDecode(responseBody.body)['data']);

    if (decryptedBody['accessToken'] != null) {
      await _storage.write(
        key: "access_token",
        value: decryptedBody['accessToken'],
      );
      decryptedBody.remove('accessToken ');
    }

    if (decryptedBody['refreshToken'] != null) {
      await _storage.write(
        key: "refresh_token",
        value: decryptedBody['refreshToken'],
      );
      decryptedBody.remove('refreshToken');
    }

    return http.StreamedResponse(
      Stream.fromIterable([utf8.encode(jsonEncode(decryptedBody))]),
      response.statusCode,
      headers: response.headers,
      request: request,
      isRedirect: response.isRedirect,
      persistentConnection: response.persistentConnection,
      contentLength: decryptedBody.length,
      reasonPhrase: response.reasonPhrase,
    );
  }
}
