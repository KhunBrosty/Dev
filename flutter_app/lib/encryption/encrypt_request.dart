import 'package:http/http.dart' as http;
import 'package:encrypt/encrypt.dart';
import 'dart:convert';

class EncryptedHttpClient extends http.BaseClient {
  final http.Client _inner = http.Client();
  final String keyString = 'AyoZNg948mTYJ6uJSlzxcbonCboWYhKL';
  final String ivString = 'Z163xZu5gheONuff';

  String encryptData(String plainText) {
    final key = Key.fromUtf8(keyString);
    final iv = IV.fromUtf8(ivString);
    final encrypter = Encrypter(AES(key, mode: AESMode.cbc, padding: 'PKCS7'));

    final encrypted = encrypter.encrypt(plainText, iv: iv);
    return encrypted.base64;
  }

  String decryptData(String encryptedText) {
    final key = Key.fromUtf8(keyString);
    final iv = IV.fromUtf8(ivString);
    final encrypter = Encrypter(AES(key, mode: AESMode.cbc, padding: 'PKCS7'));

    return encrypter.decrypt64(encryptedText, iv: iv);
  }

  @override
  Future<http.StreamedResponse> send(http.BaseRequest request) async {
    if (request is http.Request && request.body.isNotEmpty) {
      final encryptedBody = encryptData(request.body);
      request.body = jsonEncode({'data': encryptedBody});
    }

    final response = await _inner.send(request);
    return response;
  }
}
