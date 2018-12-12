/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import React from 'react';
import { RegisteredStyle, TextStyle, TouchableOpacity, View, ViewStyle } from 'react-native';
import { StyledText } from '../override/styled-text';
import radioButtonsGroupStyles from './radio-buttons-group.styles';

//============================================================================
export interface RadioButton {
    selectionIndex: number;
    label: string;

    labelStyle?: TextStyle | RegisteredStyle<TextStyle>;
    color?: string;
    disabled?: boolean;
    layout?: 'column' | 'row';
    selected?: boolean;
    size?: number;
}

//============================================================================
interface RadioButtonProps extends RadioButton {
    onPress: (label: string) => void;
}

//============================================================================
class RadioButtonComponent extends React.Component<RadioButtonProps> {

    //----------------------------------------------------------------------------
    public render() {
        const opacity = this.props.disabled ? 0.2 : 1;

        let layout: RegisteredStyle<ViewStyle>;
        let textStyle: RegisteredStyle<TextStyle>;
        if (this.props.layout === 'column') {
            layout = radioButtonsGroupStyles.view.layoutColumn;
            textStyle = radioButtonsGroupStyles.view.textColumn;
        } else {
            layout = radioButtonsGroupStyles.view.layoutRow;
            textStyle = radioButtonsGroupStyles.view.textRow;
        }

        return (
            <TouchableOpacity
                style={[layout, { opacity }]}
                onPress={() => {
                    if (!this.props.disabled) {
                        this.props.onPress(this.props.label);
                    }
                }}>
                <View
                    style={[
                        radioButtonsGroupStyles.view.radioButtonBorder,
                        {
                            borderColor: this.props.color,
                            width: this.props.size,
                            height: this.props.size,
                            borderRadius: this.props.size! / 2,
                        },
                    ]}>
                    {this.props.selected &&
                    <View
                        style={{
                            backgroundColor: this.props.color,
                            width: this.props.size! / 2,
                            height: this.props.size! / 2,
                            borderRadius: this.props.size! / 2,
                        }}
                    />}
                </View>
                <StyledText style={[textStyle, this.props.labelStyle]}>{this.props.label}</StyledText>
            </TouchableOpacity>
        );
    }
}

//============================================================================
interface RadioGroupProps {
    flexDirection?: string;
    radioButtons: RadioButton[];
    onPress: (data: RadioButton[]) => void;
}

//============================================================================
interface RadioGroupState {
    radioButtons: RadioButton[];
}

//============================================================================
export class RadioGroup extends React.Component<RadioGroupProps, RadioGroupState> {

    //----------------------------------------------------------------------------
    constructor(props: RadioGroupProps) {
        super(props);

        this.state = {
            radioButtons: this.validate(this.props.radioButtons),
        };
    }

    //----------------------------------------------------------------------------
    public render() {
        return (
            <View style={radioButtonsGroupStyles.view.radioButtonContainer}>
                <View style={{ flexDirection: this.props.flexDirection } as ViewStyle}>
                    {this.state.radioButtons.map(button => (
                        <RadioButtonComponent
                            key={button.label}
                            selectionIndex={button.selectionIndex}
                            label={button.label}
                            selected={button.selected}
                            color={button.color}
                            size={button.size}
                            disabled={button.disabled}
                            layout={button.layout}
                            labelStyle={button.labelStyle}
                            onPress={this.onPress}
                        />
                    ))}
                </View>
            </View>
        );
    }

    //----------------------------------------------------------------------------
    private validate(buttons: RadioButton[]) {
        let anotherButtonIsSelected = false;

        const validated: RadioButton[] = Array();
        buttons.forEach(function (button) {
            let copy: RadioButton = {
                selectionIndex: button.selectionIndex,
                label: button.label,
            };

            copy.color = button.color ? button.color : '#444';
            copy.disabled = button.disabled ? button.disabled : false;
            copy.layout = button.layout ? button.layout : 'row';
            copy.selected = button.selected ? button.selected : false;
            if (copy.selected) {
                if (anotherButtonIsSelected) {
                    copy.selected = false;
                    console.warn('Found "selected: true" for more than one button');
                } else {
                    anotherButtonIsSelected = true;
                }
            }
            copy.size = button.size ? button.size : 24;
            copy.labelStyle = button.labelStyle;

            validated.push(copy);
        });

        if (!anotherButtonIsSelected) {
            validated[0].selected = true;
        }

        return validated;
    }

    //----------------------------------------------------------------------------
    private onPress = (label: string) => {
        const radioButtons = this.state.radioButtons;
        const selectedIndex = radioButtons.findIndex(e => e.selected === true);
        const selectIndex = radioButtons.findIndex(e => e.label === label);
        if (selectedIndex !== selectIndex) {
            radioButtons[selectedIndex].selected = false;
            radioButtons[selectIndex].selected = true;
            this.setState({ radioButtons });
            this.props.onPress(this.state.radioButtons);
        }
    };
}
