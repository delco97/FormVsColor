using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoxStatus {
    EMPTY, // 0
    FORM_BLACK_CROSS, // 1
    FORM_WHITE_CIRCLE, // 2
    COLOR_WHITE_CROSS, // 3
    COLOR_BLACK_CIRCLE // 4
}

public static class BoxStatusManager {
    public static List<BoxStatus> GetColorStatuses() {
        return new List<BoxStatus> {
            BoxStatus.EMPTY,
            BoxStatus.COLOR_WHITE_CROSS,
            BoxStatus.COLOR_BLACK_CIRCLE
        };
    }
    
    public static List<BoxStatus> GetFormStatuses() {
        return new List<BoxStatus> {
            BoxStatus.EMPTY,
            BoxStatus.FORM_BLACK_CROSS,
            BoxStatus.FORM_WHITE_CIRCLE
        };
    }    
}
