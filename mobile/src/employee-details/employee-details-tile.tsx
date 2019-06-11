/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { StyleProp, TouchableOpacity, View, ViewStyle } from 'react-native';
import { Nullable } from 'types';
import React, { Component } from 'react';
import { ApplicationIcon } from '../override/application-icon';
import { StyledText } from '../override/styled-text';
import { employeeDetailsTileStyles } from './styles';

//============================================================================
export interface TileData {
    label: string;
    icon: string;
    style: ViewStyle;
    size: number;
    payload: Nullable<string>;
    onPress: Nullable<() => void>;
}

//============================================================================
interface EmployeeDetailsTileOwnProps {
    data: TileData;
}

export type EmployeeDetailsTileProps = EmployeeDetailsTileOwnProps;

//============================================================================
export class EmployeeDetailsTile extends Component<EmployeeDetailsTileProps> {

    //----------------------------------------------------------------------------
    public static Separator = class Separator extends Component<{ show: boolean }> {
        public render() {
            return (
                this.props.show ? <View style={employeeDetailsTileStyles.separator}/> : null
            );
        }
    };

    //----------------------------------------------------------------------------
    public render() {
        const tile = this.props.data;

        return (
            <View style={employeeDetailsTileStyles.container}>
                {
                    tile.payload !== null ?

                        <TouchableOpacity onPress={tile.onPress ? tile.onPress : undefined}>
                            <View style={this.tileStyle(tile.onPress === null)}>
                                <View style={employeeDetailsTileStyles.iconContainer}>
                                    <ApplicationIcon name={tile.icon} size={tile.size} style={tile.style}/>
                                </View>
                                <StyledText style={employeeDetailsTileStyles.text}>{tile.label}</StyledText>
                            </View>
                        </TouchableOpacity>

                        :

                        <View style={this.tileStyle(tile.onPress === null)}>
                            <View style={employeeDetailsTileStyles.iconContainer}>
                                <ApplicationIcon name={tile.icon} size={tile.size} style={tile.style}/>
                            </View>
                            <StyledText style={employeeDetailsTileStyles.text}>{tile.label}</StyledText>
                        </View>
                }
            </View>
        );
    }

    //----------------------------------------------------------------------------
    private tileStyle = (transparent: boolean): StyleProp<ViewStyle> => {
        if (transparent) {
            return [employeeDetailsTileStyles.tile, { backgroundColor: 'transparent' }];
        } else {
            return employeeDetailsTileStyles.tile;
        }
    };
}
