from django.shortcuts import render
from django.http import HttpResponse
from api_key.urls import *
from .models import *
from user.models import *
import random
from django.views.decorators.csrf import csrf_exempt

@csrf_exempt
def Join(request):
    def CreateUser(name, api_key_instance):
        new_user = User.objects.create(name=name, access_token=GenerateToken(), api_key=api_key_instance)
        api_key_instance.used = True
        api_key_instance.save(update_fields=['used'])
        return new_user

    def CreateAPIkey():
        return APIkey.objects.create(serial="11111-22222-33333-44444-55552", paid=False, used=False)

    def GenerateToken(length=32):
        return "".join(
            [random.choice("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789") for _ in range(length)])

    api_key = request.GET.get("api_key", "")
    name = request.GET.get("name", "default name")
    if api_key:
        api_key_instance = isExistAPIkey(api_key)
        if api_key_instance:
            api_key_instance = api_key_instance[0]
            if api_key_instance.used:
                return HttpResponse("이미 사용된 api-key 입니다")
            new_user = CreateUser(name, api_key_instance)
            if api_key_instance.paid:
                return HttpResponse("유료 인증 {}".format(new_user.access_token))
            else:
                return HttpResponse("사전 인증 {}".format(new_user.access_token))
        else:
            return HttpResponse("올바르지 않은 api-key 입니다")
    else:
        new_user = CreateUser(name, CreateAPIkey())
        return HttpResponse("새로 생성 {}".format(new_user.access_token))