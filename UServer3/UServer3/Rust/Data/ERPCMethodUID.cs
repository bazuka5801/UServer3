using System;

namespace UServer3.Rust.Data
{
    public enum ERPCMethodUID : UInt32
    {
        OnPlayerLanded =             2248815946, // падение игрока
        FinishLoading =              1052678473,
        StartLoading =               2832517786,
        OnModelState =               3470646823,
        UpdateMetabolism =           2310938162,
        BroadcastSignalFromClient =  555001694,
        PlayerAttack =               148642921,  // конец атаки инструментами
        CLProject =                  386279056,  // начало атаки оружием
        OnProjectileAttack =         3322107216, // конец атаки оружием
        UpdateLoot =                 3999757041, // с сервера: при перемещении в инвентаре который лутается (открытив в т.ч.)
        MoveItem =                   4191184484, // с клиента: при перемещении
        AddUI =                      92660469,
        DestroyUI =                  1986762766,
        EnterGame =                  1052678473,
        UpdatedItemContainer =       241499635,
        ForcePositionTo =            4247659151,
        Pickup =                     3306490492, // поднятие ресурсов (гриб...)
        ItemCmd =                    2116208967, // съедание
        UseSelf =                    4147870035, // лечение
        KeepAlive =                  1739731598, // начало поднятия
        Assist =                     540658179,  // само поднятия
        StartReload =                3302290555, //начало перезарядки
        Reload =                     3360326041, //конец перезарядки
        ClientKeepConnectionAlive =  2177997023, // Desync time
    }
}