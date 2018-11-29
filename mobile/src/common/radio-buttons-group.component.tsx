import React from 'react';
import { TextStyle, TouchableOpacity, View, ViewStyle } from 'react-native';
import { StyledText } from '../override/styled-text';
import { radioButtonsGroupStyles } from './radio-buttons-group.styles';

export interface RadioButton {
    selectionIndex: number;
    label: string;

    labelStyle?: TextStyle;
    color?: string;
    disabled?: boolean;
    layout?: string;
    selected?: boolean;
    size?: number;
}

interface RadioButtonProps extends RadioButton {
    onPress: (label: string) => void;
}

class RadioButtonComponent extends React.Component<RadioButtonProps> {

    public render() {
        const opacity = this.props.disabled ? 0.2 : 1;

        let layout: ViewStyle = { flexDirection: 'row' };
        let margin: ViewStyle = { marginLeft: 10 };
        if (this.props.layout === 'column') {
            layout = { alignItems: 'center' };
            margin = { marginTop: 10 };
        }
        let textStyle: TextStyle = {
            ...margin,
            alignSelf: 'center',
        };
        textStyle = {
            ...textStyle,
            ...this.props.labelStyle,
        };

        return (
            <TouchableOpacity
                style={[layout, { opacity, marginHorizontal: 10, marginVertical: 5 }]}
                onPress={() => {
                    if (!this.props.disabled) {
                        this.props.onPress(this.props.label);
                    }
                }}>
                <View
                    style={[
                        radioButtonsGroupStyles.radioButtonBorder,
                        {
                            borderColor: this.props.color,
                            width: this.props.size,
                            height: this.props.size,
                            borderRadius: this.props.size / 2,
                            alignSelf: 'center'
                        },
                    ]}>
                    {this.props.selected &&
                    <View
                        style={{
                            backgroundColor: this.props.color,
                            width: this.props.size / 2,
                            height: this.props.size / 2,
                            borderRadius: this.props.size / 2,
                        }}
                    />}
                </View>
                <StyledText style={textStyle}>{this.props.label}</StyledText>
            </TouchableOpacity>
        );
    }
}

interface RadioGroupProps {
    flexDirection?: string;
    radioButtons: RadioButton[];
    onPress: (data: RadioButton[]) => void;
}

interface RadioGroupState {
    radioButtons: RadioButton[];
}

export class RadioGroup extends React.Component<RadioGroupProps, RadioGroupState> {

    constructor(props: RadioGroupProps) {
        super(props);

        this.state = {
            radioButtons: this.validate(this.props.radioButtons),
        };
    }

    public render() {
        return (
            <View style={radioButtonsGroupStyles.radioButtonContainer}>
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

    private validate(buttons: RadioButton[]) {
        let selected = false; // Variable to check if "selected: true" for more than one button.
        buttons.map(button => {
            button.color = button.color ? button.color : '#444';
            button.disabled = button.disabled ? button.disabled : false;
            button.layout = button.layout ? button.layout : 'row';
            button.selected = button.selected ? button.selected : false;
            if (button.selected) {
                if (selected) {
                    button.selected = false; // Making "selected: false", if "selected: true" is assigned for more than one button.
                    console.log('Found "selected: true" for more than one button');
                } else {
                    selected = true;
                }
            }
            button.size = button.size ? button.size : 24;
        });
        if (!selected) {
            buttons[0].selected = true;
        }
        return buttons;
    }

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
